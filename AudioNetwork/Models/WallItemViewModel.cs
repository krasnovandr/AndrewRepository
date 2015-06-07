using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer.Models;

namespace AudioNetwork.Models
{
    public class WallItemViewModel
    {
        public int WallItemId { get; set; }
        public string Note { get; set; }
        public DateTime AddDate { get; set; }
        public string AddByUserId { get; set; }
        public string IdUserWall { get; set; }

        public string AddDateFormatted
        {
            get
            {
                return AddDate.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public List<SongViewModel> ItemSongs { get; set; }
        public UserViewModel AddByUser { get; set; }
        //public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}