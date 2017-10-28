using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Entity
{
    public partial class Template
    {
        [NotMapped]
        public string FilePath => Path.Combine(ConfigurationManager.AppSettings["TemplateRootFilePath"], FileName);
    }
}
