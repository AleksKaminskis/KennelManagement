using System;
using System.Threading.Tasks;

namespace Client.Services
{
    // Simple pub/sub service used to notify components to refresh data
    public class RefreshService
    {
        public event Func<Task>? OnChange;

        public async Task NotifyAsync()
        {
            if (OnChange != null)
            {
                await OnChange.Invoke();
            }
        }
    }
}
