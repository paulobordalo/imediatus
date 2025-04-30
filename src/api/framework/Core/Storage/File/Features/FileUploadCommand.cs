﻿using MediatR;

namespace imediatus.Framework.Core.Storage.File.Features;

public class FileUploadCommand : IRequest<FileUploadResponse>
{
    public string Name { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string Data { get; set; } = default!;
}

