using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Data.Models
{
    public class GlossaryWordItem
    {
        public long Id { get; set; } = 0;

        /// <summary>
        /// 单词名称
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 音标，以英语英标为主
        /// </summary>
        public string Phonetic { get; set; }

        /// <summary>
        /// 单词释义（英文），每行一个释义
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// 单词释义（中文），每行一个释义
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// 时态复数等变换，使用 "/" 分割不同项目
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 生词本备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 生词本颜色
        /// </summary>
        public int Color { get; set; } = 0;
    }
}
