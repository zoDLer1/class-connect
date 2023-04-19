using PracticeWeb.Services.FileSystemServices.Helpers;

namespace PracticeWeb.Services;

public delegate IFileSystemHelper ServiceResolver(Type key);
