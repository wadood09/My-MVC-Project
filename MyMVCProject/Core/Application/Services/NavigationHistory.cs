using System.Text.Json;

namespace MyMVCProject.Core.Application.Services
{
    public class NavigationHistoryItem
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public class NavigationHistoryManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string NavigationHistoryKey = "NavigationHistory";

        public NavigationHistoryManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToHistory(string controllerName, string actionName, Dictionary<string, string> parameters = null)
        {
            var history = _httpContextAccessor.HttpContext.Session.GetString(NavigationHistoryKey);
            var historyList = history != null ? JsonSerializer.Deserialize<List<NavigationHistoryItem>>(history) : new List<NavigationHistoryItem>();
            if(historyList.Count > 15)
            {
                historyList.RemoveRange(0, 10);
            }
            historyList.Add(new NavigationHistoryItem
            {
                ControllerName = controllerName,
                ActionName = actionName,
                Parameters = parameters ?? new Dictionary<string, string>()
            });
            _httpContextAccessor.HttpContext.Session.SetString(NavigationHistoryKey, JsonSerializer.Serialize(historyList));
        }

        public NavigationHistoryItem GetPreviousView(int index = 2)
        {
            var history = _httpContextAccessor.HttpContext.Session.GetString(NavigationHistoryKey);
            var historyList = history != null ? JsonSerializer.Deserialize<List<NavigationHistoryItem>>(history) : new List<NavigationHistoryItem>();
            if (historyList.Count >= 2)
            {
                // Get the second last item (previous view)
                historyList.RemoveAt(historyList.Count - 1);
                return historyList[historyList.Count - (index - 1)];
            }
            return null;
        }

        public NavigationHistoryItem GetSameView()
        {
            var history = _httpContextAccessor.HttpContext.Session.GetString(NavigationHistoryKey);
            var historyList = history != null ? JsonSerializer.Deserialize<List<NavigationHistoryItem>>(history) : new List<NavigationHistoryItem>();
            if (historyList.Count >= 2)
            {
                // Get the second last item (previous view)
                return historyList[historyList.Count - 1];
            }
            return null;
        }
    }
}
