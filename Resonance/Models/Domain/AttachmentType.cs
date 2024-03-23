﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AttachmentType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Attachment> Attachments { get; } = new List<Attachment>();
}