using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Models
{
    public class GlossaryExModel : GlossaryModelBase
    {
        /// <summary>
        /// 内置生词本的内部标签，目前有zk(中考)、gk(高考)、ky(考研)、cet4、cet6、toefl(托福)、ielts(雅思)、gre、oxford等
        /// 注意在数据库中，oxford是单独一个字段
        /// </summary>
        public string ExtraGlossaryInternalTag { get; set; } = string.Empty;
    }
}
