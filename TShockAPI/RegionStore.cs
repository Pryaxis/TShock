using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI.DB;

namespace TShockAPI
{
    class RegionStore
    {
        public int X1;
        public int X2;
        public int Y1;
        public int Y2;
        public int IsProtected;
        public string[] UserIDs;
        public RegionStore(string tX1, string tX2, string tY1, string tY2, int tIsProtected, string[] tUserIDs)
        {
            X1 = Convert.ToInt32(tX1);
            X2 = Convert.ToInt32(tX2);
            Y1 = Convert.ToInt32(tY1);
            Y2 = Convert.ToInt32(tY2);
            IsProtected = tIsProtected;
            UserIDs = tUserIDs;
        }

        public RegionStore(int tX1, int tX2, int tY1, int tY2, int tIsProtected, string[] tUserIDs)
        {
            X1 = Convert.ToInt32(tX1);
            X2 = Convert.ToInt32(tX2);
            Y1 = Convert.ToInt32(tY1);
            Y2 = Convert.ToInt32(tY2);
            IsProtected = tIsProtected;
            UserIDs = tUserIDs;
        }

        public RegionStore(string tX1, string tX2, string tY1, string tY2)
        {
            X1 = Convert.ToInt32(tX1);
            X2 = Convert.ToInt32(tX2);
            Y1 = Convert.ToInt32(tY1);
            Y2 = Convert.ToInt32(tY2);
        }

        public RegionStore(int tX1, int tX2, int tY1, int tY2)
        {
            X1 = Convert.ToInt32(tX1);
            X2 = Convert.ToInt32(tX2);
            Y1 = Convert.ToInt32(tY1);
            Y2 = Convert.ToInt32(tY2);
        }

        public bool InProtectedArea(int x, int y, User user)
        {
            if (x >= X1 &&
                x <= X2 &&
                y >= Y1 &&
                y <= Y2 &&
                IsProtected == 1)
            {
                if (!UserIDs.Contains(user.ID.ToString()))
                    return true;
            }
            return false;
        }
    }
}
