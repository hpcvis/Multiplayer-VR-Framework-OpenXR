// i have no idea how audio syncing is going to work

// List of things to do:
// 1. [DONE] Send lip weights and set them over the network
// 2. [DONE] Smooth/Lerp lip weights to account for lag
// 3. [DONE] Account for multiple LipShapeTables (like the Lip Sample does)
// 4. [Currently Working On] Draw the rest of the owl (sync to audio)
// 5. (Bonus!) Return to step 2 to account for audio lag

// Other Notes:
// Queue and serialization modified from the way Photon View syncs animations, except much simpler (for now)

namespace Photon.Pun
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using ViveSR.anipal.Lip;
    using Photon.Voice.PUN;

    public class PhotonBlendshapeView : MonoBehaviourPun, IPunObservable
    {
        #region Variables
        //Defaults
        private Dictionary<int, float> DS_LIP_SHAPE_DEFAULTS;

        // Lip Shape Related
        private SRanipal_AvatarLipSample_v2 LipBehavior;
        private List<LipShapeTable_v2> LipShapeTables;
        private Dictionary<LipShape_v2, float> LipWeightings;

        // Audio Related
        public bool SyncAudio;
        private bool m_SyncAudio;

        private GameObject NetworkedPlayer;
        private PhotonVoiceView NetworkedVoice;

        // Serialization Related
        PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120); // same sampling rate as animation for now, modify for smoothness later
        private string LipShapeSerialized;
        private Dictionary<int, float> LipShapeDeserialized;

        // Debug
        bool test;
        bool isFacialTrackingWorking;
        byte[] testArray;

        #endregion

        #region Unity
        public void Awake()
        {
            //find lip behavior script attached to model
            this.LipBehavior = this.GetComponent(typeof(SRanipal_AvatarLipSample_v2)) as SRanipal_AvatarLipSample_v2;
            if (this.LipBehavior == null)
            {
                Debug.LogError("PhotonBlendShapeView::Awake(): Lip tracking behavior is missing; attach SRanipal_AvatarLipSample_v2 script to " + this.gameObject.name);
            }

            this.LipShapeTables = this.LipBehavior.GetLipShapeTables();

            // set up default lipshape dictionary<int, float> based on attached lip table(s)
            DS_LIP_SHAPE_DEFAULTS = new Dictionary<int, float>();
            for (int i = 0; i < this.LipShapeTables[0].lipShapes.Length; i++)
            {
                DS_LIP_SHAPE_DEFAULTS.Add(i, 0f);
            }

            // set starting values
            SRanipal_Lip_v2.GetLipWeightings(out this.LipWeightings);
            this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);
            this.LipShapeDeserialized = StringToLipWeights(this.LipShapeSerialized);
        }

        void Start()
        {
            // find photon voice view in VoiceConnection
            // start() goes after awake(); photon voice view is created in awake()
            this.NetworkedPlayer = this.transform.root.gameObject;
            if (this.NetworkedPlayer != null)
            {
                this.NetworkedVoice = this.NetworkedPlayer.GetComponentInChildren(typeof(PhotonVoiceView)) as PhotonVoiceView;

                if (this.NetworkedVoice == null)
                {
                    Debug.LogError("PhotonBlendshapeView::Start(): Networked Player " + this.transform.root.gameObject.name + " does not have a PhotonVoiceView in children. Audio Sync is disabled.");
                    this.SyncAudio = false;
                }
            }

            this.m_SyncAudio = this.SyncAudio;
        }

        void Update()
        {
            this.isFacialTrackingWorking = (SRanipal_Lip_Framework.Status == SRanipal_Lip_Framework.FrameworkStatus.WORKING);

            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                // don't queue up data with nowhere to go
                this.m_StreamQueue.Reset();

                // for local rendering when alone
                this.LipBehavior.UpdateLipShapes(this.LipBehavior.GetLipWeightingsDict());
                return;
            }

            // serialize data
            if (this.photonView.IsMine && this.m_SyncAudio)
            {
                this.SerializeDataContinuouslyAudio();
                this.LipBehavior.UpdateLipShapes(this.LipBehavior.GetLipWeightingsDict());
            }
            else if (this.photonView.IsMine)
            {
                this.SerializeDataContinuously();
                this.LipBehavior.UpdateLipShapes(this.LipBehavior.GetLipWeightingsDict());
            }
            else if (!this.photonView.IsMine && this.m_SyncAudio)
            {
                this.DeserializeDataContinuouslyAudio();
            }
            else //!this.photonView.IsMine
            {
                this.DeserializeDataContinuously();
            }
        }

        #endregion

        #region Serialization

        // serialize continuously for smooth lip movements
        private void SerializeDataContinuously()
        {
            if (this.LipBehavior == null || !this.isFacialTrackingWorking)
            {
                return;
            }

            //only want one lip weight dictionary sent at a time to set for all tables
            this.LipWeightings = this.LipBehavior.GetLipWeightingsDict();
            this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);

            byte[] temp = Encoding.UTF8.GetBytes(LipShapeSerialized);
            this.m_StreamQueue.SendNext(temp);
        }

        private void DeserializeDataContinuously()
        {
            if (!this.m_StreamQueue.HasQueuedObjects())
            {
                // nothing to deserialize, render default blendshapes (all 0's)
                foreach (var table in this.LipShapeTables)
                    RenderLipShapeNetwork(table, DS_LIP_SHAPE_DEFAULTS);

                return;
            }

            string LipFromBytes = Encoding.UTF8.GetString((byte[])this.m_StreamQueue.ReceiveNext());
            this.LipShapeDeserialized = StringToLipWeights(LipFromBytes);

            foreach (var table in this.LipShapeTables)
                RenderLipShapeNetwork(table, this.LipShapeDeserialized);
        }

        // for testing audio sync
        private void SerializeDataContinuouslyAudio()
        {
            if (this.LipBehavior == null || this.NetworkedVoice == null || !this.isFacialTrackingWorking)
            {
                return;
            }

            if (!this.NetworkedVoice.RecorderInUse.TransmitEnabled)
            {
                return;
            }

            // hypothetically should only grab lip movements when recording
            if (this.NetworkedVoice.RecorderInUse.TransmitEnabled)
            {
                this.LipWeightings = this.LipBehavior.GetLipWeightingsDict();
                this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);

                byte[] temp = Encoding.UTF8.GetBytes(LipShapeSerialized);
                this.m_StreamQueue.SendNext(temp);
            }

            return;
        }

        private void DeserializeDataContinuouslyAudio()
        {
            if (!this.m_StreamQueue.HasQueuedObjects())
            {
                foreach (var table in this.LipShapeTables)
                    RenderLipShapeNetwork(table, DS_LIP_SHAPE_DEFAULTS);

                return;
            }
            
            // not ideal if recorder can be enabled and disabled at will
            // i assume if the audio stops playing and the frames aren't finished, queue will build up with unwanted weights
            // need to find a way to reset the queue after the speaker stops playing
            if (this.NetworkedVoice.SpeakerInUse.IsPlaying)
            {
                string LipFromBytes = Encoding.UTF8.GetString((byte[])this.m_StreamQueue.ReceiveNext());
                this.LipShapeDeserialized = StringToLipWeights(LipFromBytes);

                foreach (var table in this.LipShapeTables)
                    RenderLipShapeNetwork(table, this.LipShapeDeserialized);
            }

            return;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.LipBehavior == null)
            {
                return;
            }

            if (stream.IsWriting) // Write
            {
                if (this.m_SyncAudio != this.SyncAudio)
                {
                    // reset the queue if audio sync option has changed to prevent build up of queue
                    // could be used to switch between tracking with audio and tracking without it fluidly (little extra thing for me to do i guess)
                    this.m_StreamQueue.Reset();
                    this.m_SyncAudio = this.SyncAudio;
                }

                this.m_StreamQueue.Serialize(stream);
            }
            else // Read
            {
                this.m_StreamQueue.Deserialize(stream);
            }
        }

        #endregion

        #region Helpers

        // would JSON for serialization but Unity's JSON doesn't support dictionaries so
        // path of least resistance here

        /// <summary>
        /// Converts a dictionary of lip weightings into a string for serialization.
        /// </summary>
        /// <param name="weighting">Lip weights</param>
        /// <returns>A string consisting of {key,value}{key,value}...</returns>
        public string LipWeightsToString(Dictionary<LipShape_v2, float> weighting)
        {
            var lipString = new StringBuilder();

            for (int i = 0; i < this.LipShapeTables[0].lipShapes.Length; i++)
            {
                int targetIndex = (int) this.LipShapeTables[0].lipShapes[i];
                if (targetIndex > (int)LipShape_v2.Max || targetIndex < 0) continue;

                // write to string
                lipString.Append('{');
                lipString.Append(string.Format("{0}={1}", i, weighting[(LipShape_v2)targetIndex] * 100));
                lipString.Append('}');
            }

            return lipString.ToString();
        }

        /// <summary>
        /// Deserializes a string of lip weights back into a dictionary.
        /// </summary>
        /// <param name="weighting">Serialized lip weights</param>
        /// <returns>A dictionary of int, float</returns>
        public Dictionary<int, float> StringToLipWeights(string weighting)
        {
            var weights = weighting.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries).Select(w => w.Split(new[] { '=' }));
            Dictionary<int, float> weightsDict = new Dictionary<int, float>();
            foreach (var w in weights)
            {
                weightsDict.Add(int.Parse(w[0]), float.Parse(w[1]));
            }
            return weightsDict;
        }


        /// <summary>
        /// Renders blend shape weights on all lip shape tables on a model
        /// </summary>
        /// <param name="table">Lip shape table that is being modified</param>
        /// <param name="weightings">Lip weightings to set blend shape weight</param>
        public void RenderLipShapeNetwork(LipShapeTable_v2 table, Dictionary<int, float> weightings)
        {
            for (int i = 0; i < table.lipShapes.Length; i++)
            {
                table.skinnedMeshRenderer.SetBlendShapeWeight(i, weightings[i]);
            }
        }

        #endregion

    }
}
