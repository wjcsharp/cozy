﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CozyNote.Model.ObjectModel;

namespace CozyNote.Model.APIModel.Output
{
    public class NoteGetOutput : OutputBase
    {
        public Note Result { get; set; }
    }
}
