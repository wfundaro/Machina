﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Model
{
    public class FaceDetectModels
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }
        public class FaceRectangle
        {
            public int top { get; set; }
            public int left { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class FaceAttributes
        {
            public string gender { get; set; }
            public double age { get; set; }
        }
    }
}
