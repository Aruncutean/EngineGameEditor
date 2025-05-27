using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.services
{
    public class WindowsService
    {
        private static WindowsService instance = null;
        public int Width { get; set; } = 1200;
        public int Height { get; set; } = 600;

        private WindowsService()
        {
        }

        public static WindowsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowsService();
                }
                return instance;
            }
        }
    }
}
