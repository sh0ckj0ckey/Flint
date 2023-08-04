using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Models
{
    public class GlossaryBuildinModel
    {
        /// <summary>
        /// 生词本的颜色
        /// </summary>
        public GlossaryColorsEnum GlossaryColor { get; set; } = GlossaryColorsEnum.Transparent;

        /// <summary>
        /// 生词本名称
        /// </summary>
        public string GlossaryTitle { get; set; } = string.Empty;

        /// <summary>
        /// 目前用来显示生词本内单词数量
        /// </summary>
        public string GlossaryDescription { get; set; } = string.Empty;

        /// <summary>
        /// 内置生词本的图标
        /// </summary>
        public string BuildinGlossaryIcon { get; set; } = string.Empty;

        /// <summary>
        /// 内置生词本的内部标签，目前有zk(中考)、gk(高考)、ky(考研)、cet4、cet6、toefl(托福)、ielts(雅思)、gre、oxford等
        /// 注意在数据库中，oxford是单独一个字段
        /// </summary>
        public string BuildinGlossaryInternalTag { get; set; } = string.Empty;
    }
}
