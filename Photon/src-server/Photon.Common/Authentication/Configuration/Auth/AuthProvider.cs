using System.Collections.Generic;
using Photon.Common.Authentication.Data;

namespace Photon.Common.Authentication.Configuration.Auth
{
    public class AuthProvider
    {
        #region Properties

        public Dictionary<string, string> CustomAttributes { get; } = new Dictionary<string, string>();

        public string Name { get; set; } = "";

        public string AuthUrl { get; set; } = "";

        public int AuthenticationType { get; set; } = 0;

        public bool RejectIfUnavailable { get; set; } = true;

        public bool ForwardAsJSON { get; set; } = false;

        public string NameValuePairAsQueryString { get; private set; }

        #endregion

        public override string ToString()
        {
            return string.Format("[Name={0},type={4},url={1},RejectIfUnavailable={2},keys={3}]",
                this.Name,
                this.AuthUrl,
                this.RejectIfUnavailable,
                this.NameValuePairAsQueryString,
                (ClientAuthenticationType)this.AuthenticationType);
        }

        #region Methods

        public void PostDeserialize()
        {
            this.NameValuePairAsQueryString = GetNameValuePairsAsQueryString(this.CustomAttributes);
        }

        private static string GetNameValuePairsAsQueryString(Dictionary<string, string> nameValuePairs)
        {
            if (nameValuePairs == null || nameValuePairs.Count == 0) return null;

            var httpValueCollection = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (var entry in nameValuePairs)
            {
                httpValueCollection.Add(entry.Key, entry.Value);
            }
            return httpValueCollection.ToString();
        }


        #endregion
    }
}
