using System;
using System.Collections;
using System.Text;


namespace Codice.Client.Extension.Types
{

    public class RedmineExtensionConfiguration : BaseExtensionConfiguration
    {
        public string Url = string.Empty;
        public string ShowIssueUrl = string.Empty;
        public string IssueWeb = "/{0}";

        // You can find your API key on your account page ( /my/account ) when logged in, 
        // on the right-hand pane of the default layout.
        public string ApiKey = string.Empty;
        public string User = string.Empty;
        public string Password = string.Empty;
        public string RepositoryName = string.Empty;
    }

    public class RedmineExtensionMultipleConfiguration
    {
        public ExtensionWorkingMode WorkingMode;
        public RedmineExtensionConfiguration[] ConfigurationEntries;

        public RedmineExtensionMultipleConfiguration()
        {
            WorkingMode = ExtensionWorkingMode.TaskOnBranch;
            ConfigurationEntries = new RedmineExtensionConfiguration[] { new RedmineExtensionConfiguration(), new RedmineExtensionConfiguration() };
        }

        public string GetUrl(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].Url;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.Url;
            }

            return ConfigurationEntries[0].Url;
        }

        public string GetShowIssueUrl(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].ShowIssueUrl;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.ShowIssueUrl;
            }

            return ConfigurationEntries[0].ShowIssueUrl;
        }

        public string GetIssueWeb(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].IssueWeb;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.IssueWeb;
            }

            return ConfigurationEntries[0].IssueWeb;
        }

        public string GetApiKey(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].ApiKey;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.ApiKey;
            }

            return ConfigurationEntries[0].ApiKey;
        }

        public string GetUser(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].User;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.User;
            }

            return ConfigurationEntries[0].User;
        }

        public string GetPassword(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].Password;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.Password;
            }

            return ConfigurationEntries[0].Password;
        }

        public string GetBranchPrefix(string repName)
        {
            if (ConfigurationEntries.Length == 1)
                return ConfigurationEntries[0].BranchPrefix;

            foreach (RedmineExtensionConfiguration config in ConfigurationEntries)
            {
                if (IsRepositoryIncluded(config.RepositoryName, repName))
                    return config.BranchPrefix;
            }

            return ConfigurationEntries[0].BranchPrefix;
        }

        private bool IsRepositoryIncluded(string configRep, string repName)
        {
            if (configRep == null || configRep == string.Empty || repName == null || repName == string.Empty)
                return false;

            string[] reps = configRep.Split(new char[] { ';' });
            foreach (string rep in reps)
            {
                if (repName == rep.Trim())
                    return true;
            }

            return false;
        }
    }
}


