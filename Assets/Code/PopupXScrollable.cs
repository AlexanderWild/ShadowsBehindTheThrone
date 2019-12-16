using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public interface PopupXScrollable
    {
        float xSize();
        string getTitle();
        string getBody();
        void setTargetX(float y);
    }
}
