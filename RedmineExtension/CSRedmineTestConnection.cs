using System;
using Redmine.Net.Api.Types;
using Redmine.Net.Api;
using Codice.Utils;

namespace Codice.Client.IssueTracker.RedmineExtension
{
    public class CSRedmineTestConnection
    {
        private static string[] AUTH_EXCEPTION =
            new string[]{"Authorization Required",
                         "Unauthorized",
                         "Timeout",
                         "The operation has timed-out.",
                         "The underlying connection was closed: " +
                         "Unable to connect to the remote server."};

        public CSRedmineTestConnection()
        {
        }

        public bool CsTestConnection(IssueTrackerConfiguration configuration)
        {
            IssueTrackerConfiguration redmineConfig = configuration as IssueTrackerConfiguration;

            try
            {
                var manager = CreateRedmineManager(redmineConfig);
                User user = manager.GetCurrentUser();
                return !string.IsNullOrEmpty(user.Login);
            }
            catch(Exception)
            {
                throw;
            }

        }



        private static RedmineManager CreateRedmineManager(IssueTrackerConfiguration config)
        {
            string host = config.GetValue(CSRedmineExtension.HOST_KEY); 
            string apikey = config.GetValue(CSRedmineExtension.APIKEY_KEY); 
            if (apikey == null || apikey == String.Empty)
            {
                string user = config.GetValue(CSRedmineExtension.USER_KEY);  //  config.User;
                string password = CryptoServices.GetDecryptedPassword(config.GetValue(CSRedmineExtension.PASSWORD_KEY));

                return new RedmineManager(host, user, password);
            }

            return new RedmineManager(host, apikey);
        }
    }
}

