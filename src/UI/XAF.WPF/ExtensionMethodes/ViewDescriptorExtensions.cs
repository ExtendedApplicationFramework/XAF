﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAF.UI.Abstraction;
using XAF.UI.Abstraction.Attributes;

namespace XAF.UI.WPF.ExtensionMethodes;
public static class ViewDescriptorExtensions
{
    public static IEnumerable<object> GetNavigationKeys(this ViewDescriptor viewDescriptor)
    {
		try
		{
			return viewDescriptor.GetDecoratorValue<ContainsViewContainerAttribute, HashSet<object>>();
        }
        catch
		{
			return Enumerable.Empty<object>();
		}
    }
}
