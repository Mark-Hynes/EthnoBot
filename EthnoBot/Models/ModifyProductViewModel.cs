using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Models
{
    public class ModifyProductViewModel
    { public List<Category> Categories;
        public Product Product {get;set;}
      
        public string selectedCategoryId;
        public IEnumerable<SelectListItem> CategoryListItems
        {
            get { return new SelectList(Categories, "CategoryId", "Name"); }
        }

    }
}