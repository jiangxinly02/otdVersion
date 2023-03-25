using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otdVision.Entity
{
    public class ProcessingRule
    {
        public bool IsGray { get; set; } = true;

        public string FilterName { get; set; }

        public string FilterParamater { get; set; }

        public string DetectionName { get; set; }

        public string DetectionParamater { get; set; }

        public string FilePath { get; set; }
    }
}
