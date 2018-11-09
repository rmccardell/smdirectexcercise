using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Core.Entities
{
    /// <summary>
    /// Represents a Space X LaunchPad object
    /// </summary>
    public class LaunchPad
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

    }
}
