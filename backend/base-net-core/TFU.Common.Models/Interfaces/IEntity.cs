using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.Common
{
  public interface IEntity<DTO>
  {
    DTO GetEntity();
  }
}
