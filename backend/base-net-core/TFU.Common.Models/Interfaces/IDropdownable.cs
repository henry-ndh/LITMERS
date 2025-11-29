using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TFU.Common.Models;

namespace TFU.Common.Interfaces
{
	public interface IDropdownable
	{
		Task<List<Dropdown>> GetDropdown();
	}
}
