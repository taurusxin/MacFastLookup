using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacFastLookup
{
    public class Config
    {
        private const string CONFIG_PATH = "config.json";
        public string CheckUpdateUrl { get; set; }
        public string DownloadUrl { get; set; }
    }
}
