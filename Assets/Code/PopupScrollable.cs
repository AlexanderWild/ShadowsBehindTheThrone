using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public interface PopupScrollable
    {
        float ySize();
        void clicked(Map map);
        string getTitle();
        string getBody();
        void setTargetY(float y);
        bool overwriteSidebar();
        bool selectable();
    }
}
