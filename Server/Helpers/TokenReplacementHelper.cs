using IOGKFExams.Server.Models.IOGKFExamsDb;

namespace IOGKFExams.Server.Helpers
{
    public static class TokenReplacementHelper
    {
        public static NotificationTemplate ReplaceTokens(
            NotificationTemplate template,
            Dictionary<string, string> tokens)
        {
            if (template == null)
            {
                return template;
            }

            if (tokens == null || tokens.Count == 0)
            {
                return template;
            }

            foreach (var token in tokens)
            {
                template.Subject = template.Subject.Replace(
                    $"{{{{{token.Key}}}}}",
                    token.Value ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase);
                template.MessageBody = template.MessageBody.Replace(
                    $"{{{{{token.Key}}}}}",
                    token.Value ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase);
            }

            return template;
        }
    }
}