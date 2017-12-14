using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace N1.TheStart.Controllers
{
    /// <summary>
    /// 产品
    /// </summary>
    public class Product
    {
        [Display(Name = "产品编号")]
        public int ID { get; set; }
        [Display(Name="产品名称")]
        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(15,MinimumLength =2,ErrorMessage ="{0}的最大长度不能超过{1}，最小长度不能小于{2}")]
        public string Name { get; set; }
        [Display(Name = "价格")]
        public double Price { get; set; }
        public ICollection<Storehouse> Storehouses { get; set; }
    }
}
