using CoreBridge.Models.Entity;
using CoreBridge.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoreBridge.ViewModels
{
    public class MUserIndexViewModel
    {
        public AppUserDto AppUserDto { get; set; }
        public AppUserSearchDto? AppUserSearchDto { get; set; }
        public List<AppUserDto>? AppUserDtos { get; set; }

        public int RoleIndex { get; set; }
        public enum RoleEnum
        {
            [Display(Name = "-1：DataLake権限")]
            Manager = 2,
            [Display(Name = "1：参照権限")]
            Reference,
            [Display(Name = "2：参照権限＋編集権限")]
            EditReference
        }

        public int RoleSearchIndex { get; set; }
        public enum RoleSearchEnum
        {
            [Display(Name = "指定なし")]
            None = 0,
            [Display(Name = "-1：DataLake権限")]
            Manager = 2,
            [Display(Name = "1：参照権限")]
            Reference,
            [Display(Name = "2：参照権限＋編集権限")]
            EditReference
        }
    }
}
