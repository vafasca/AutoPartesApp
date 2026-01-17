using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Layout
{
    public partial class ClientLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        private string currentUrl = string.Empty;

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager?.Uri ?? string.Empty;
            if (NavigationManager != null)
            {
                NavigationManager.LocationChanged += OnLocationChanged;
            }
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = e.Location;
            StateHasChanged();
        }

        private string GetNavLinkClass(string href)
        {
            var isActive = IsActiveRoute(href);
            return $"nav-link-client {(isActive ? "active" : "")}";
        }

        private string GetIconFillClass(string href)
        {
            return IsActiveRoute(href) ? "icon-fill" : "";
        }

        private bool IsActiveRoute(string href)
        {
            if (NavigationManager == null) return false;

            var relativePath = NavigationManager.ToBaseRelativePath(currentUrl);
            var targetPath = href.TrimStart('/');

            return relativePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase) ||
                   relativePath.StartsWith(targetPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            if (NavigationManager != null)
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }
        }
    }
}
