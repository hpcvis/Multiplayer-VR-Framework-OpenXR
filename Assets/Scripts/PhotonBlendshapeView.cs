// this doesn't give me an error...yet
// i have no idea how audio syncing is going to work (check if isRecording and isSpeaking???)

// List of things to do:
// 1. Send lip weights and set them over the network (currently working on)
// 2. Smooth/Lerp lip weights to account for lag
// 3. Account for multiple LipShapeTables (like the Lip Sample does)
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
            this.LipShapeTables = this.LipBehavior.GetLipShapeTable();
            SRanipal_Lip_v2.GetLipWeightings(out this.LipWeightings);
            this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);
            this.LipShapeDeserialized = StringToLipWeights(this.LipShapeSerialized);

            //Debug.Log(this.LipShapeSerialized);
            /*foreach(KeyValuePair<int, float> p in this.LipShapeDeserialized)
            {
                test += string.Format("Key = {0}, Value = {1}, ", p.Key, p.Value);
            }
            Debug.Log(test);*/
        }

        // Update is called once per frame
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
                // idea: render lip shapes using dict <int, float> of lip weightings
                //RenderLipShapeNetwork(this.LipShapeDeserialized);

                this.DeserializeDataContinuously();
            }
        }

        #endregion

        #region Serialization

        // hypothetically speaking, this should be serializing continiously in order to have smooth lip movement
        // otherwise lerping might have to be a thing
        private void SerializeDataContinuously()
        {
            // if lip sample doesn't exist, serialize nothing
            if (this.LipBehavior == null)
            {
                return;
            }

            //SRanipal_Lip_v2.GetLipWeightings(out this.LipWeightings);
            this.LipWeightings = this.LipBehavior.GetLipWeightingsDict(); // hopefully more efficient than calculating lipweightings again
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
            RenderLipShapeNetwork(this.LipShapeDeserialized);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.LipBehavior == null)
            {
                return;
            }

            if (stream.IsWriting) // Write
            {
                // idea: get lip weightings, serialize them into a string, and then send to stream
                /*SRanipal_Lip_v2.GetLipWeightings(out this.LipWeightings);
                this.LipShapeSerialized = LipWeightsToString(this.LipWeightings);
                stream.SendNext(this.LipShapeSerialized);*/

                this.m_StreamQueue.Serialize(stream);
            }
            else // Read
            {
                // idea: get lip weightings via string, and deserialize them into a dict<int, float>
                /*this.LipShapeSerialized = (string)stream.ReceiveNext();
                this.LipShapeDeserialized = StringToLipWeights(this.LipShapeSerialized);*/

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
            //should work???
            var weights = weighting.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries).Select(w => w.Split(new[] { '=' }));
            Dictionary<int, float> weightsDict = new Dictionary<int, float>();
            foreach (var w in weights)
            {
                weightsDict.Add(int.Parse(w[0]), float.Parse(w[1]));
            }
            return weightsDict;
        }


        /// <summary>
        /// Renders the lips over the network by setting the deserializaed blend shapes into the skinned mesh renderer
        /// </summary>
        /// <param name="weightings">A dictionary of int, float</param>
        public void RenderLipShapeNetwork(Dictionary<int, float> weightings)
        {
            // okay, this one idk if it works or not
            // since we already calculated the target index's (lip shape) weighting, this should just plug them in order
            // hypothetically

            for (int i = 0; i < this.LipShapeTables[0].lipShapes.Length; i++)
            {
                //int targetIndex = (int)LipShapeTables[0].lipShapes[i];
                //if (targetIndex > (int)LipShape_v2.Max || targetIndex < 0) continue;
                this.LipShapeTables[0].skinnedMeshRenderer.SetBlendShapeWeight(i, weightings[i]);
            }
        }

        #endregion

    }
}
