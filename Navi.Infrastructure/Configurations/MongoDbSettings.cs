using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Infrastructure.Configurations;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ChatCollectionName { get; set; } = string.Empty;
    public string RagCollectionName { get; set; } = string.Empty;
}