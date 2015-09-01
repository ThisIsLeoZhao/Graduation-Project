namespace SingleKinect.MyDataStructures
{
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner

        public int this[string attr]
        {
            get
            {
                switch (attr)
                {
                    case "Right":
                        return Right;
                    case "Left":
                        return Left;
                    case "Bottom":
                        return Bottom;
                    case "Top":
                        return Top;
                }
                return -1;
            }
            set
            {
                switch (attr)
                {
                    case "Right":
                        Right = value;
                        break;

                    case "Left":
                        Left = value;
                        break;

                    case "Bottom":
                        Bottom = value;
                        break;

                    case "Top":
                        Top = value;
                        break;
                }
            }
        }
    }
}