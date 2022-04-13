// this doesn't give me an error...yet
// i have no idea how audio syncing is going to work (check if isRecording and isSpeaking???)

// List of things to do:
// 1. [DONE] Send lip weights and set them over the network
// 2. [DONE] Smooth/Lerp lip weights to account for lag
// 3. [Currently working on] Account for multiple LipShapeTables (like the Lip Sample does)
// 4. Draw the rest of the owl (sync to audio)
// 5. (Bonus!) Return to step 2 to account for audio lag

namespace Photon.Pun
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using ViveSR.anipal.Lip;

    public class PhotonBlendshapeView : MonoBehaviourPun, IPunObservable
    {
        #region Variables
        // Lip Shape Related
        [SerializeField] private SRanipal_AvatarLipSample_v2 LipBehavior;
        [SerializeField] private List<LipShapeTable_v2> LipShapeTables; // set at run time, remove serialize field once done debugging
        private Dictionary<LipShape_v2, float> LipWeightings;

        // Serialization Related
        PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120); // same sampling rate as animation for now
        private string LipShapeSerialized;
        private Dictionary<int, float> LipShapeDeserialized;

        // Debug
        string test;

        #endregion

        #region Unity
        public void Awake()
        {
            // similiar to other photon views, set initial variables in Awake()
            if (this.LipBehavior == null)
            {
                Debug.LogError("Lip tracking behavior is missing; attach SRanipal_AvatarLipSample_v2 to object", this.LipBehavior); // improve error message pls
            }
            else
            {
                Debug.Log("Successfully found script for lip tracking behavior");
            }
            this.LipShapeTables = this.LipBehavior.GetLipShapeTables();
            SRanipal_Lip_v2.GetLipWeightings(out this.LipWeightings);
            this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);
            this.LipShapeDeserialized = StringToLipWeights(this.LipShapeSerialized);
        }

        void Update()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                // don't queue up data with nowhere to go
                this.m_StreamQueue.Reset();
                return;
            }

            if (this.photonView.IsMine)
            {
                this.SerializeDataContinuously();
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
            // if lip sample doesn't exist, serialize nothing
            if (this.LipBehavior == null)
            {
                return;
            }

            //only want one lip weight dictionary sent at a time to set for all tables
            this.LipWeightings = this.LipBehavior.GetLipWeightingsDict(); // maybe more efficient than calculating lip weights again
            this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);
            this.m_StreamQueue.SendNext(this.LipShapeSerialized);
        }

        private void DeserializeDataContinuously()
        {
            if (!this.m_StreamQueue.HasQueuedObjects())
            {
                return;
            }

            this.LipShapeDeserialized = StringToLipWeights((string)this.m_StreamQueue.ReceiveNext());
            for (int i = 0; i < this.LipShapeTables.Count; i++)
            {
                RenderLipShapeNetwork(this.LipShapeTables[i], this.LipShapeDeserialized);
            }

            /*this.LipShapeDeserialized = StringToLipWeights((string)this.m_StreamQueue.ReceiveNext());
            RenderLipShapeNetwork(this.LipShapeDeserialized);*/
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.LipBehavior == null)
            {
                return;
            }

            if (stream.IsWriting) // Write
            {
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
