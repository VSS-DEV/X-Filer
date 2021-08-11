﻿using System.Collections.Generic;

namespace XFiler
{
    internal class BookmarkItem
    {
        public string? Path { get; set; }

        public string? BookmarkFolderName { get; set; }  

        public IList<BookmarkItem>? Children { get; set; }
    }   
}