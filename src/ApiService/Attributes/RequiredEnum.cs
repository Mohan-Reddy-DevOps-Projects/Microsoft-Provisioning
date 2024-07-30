// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Attributes;

using System;
using System.ComponentModel.DataAnnotations;

public class RequiredEnumAttribute : RequiredAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return false;
        }

        Type type = value.GetType();
        return type.IsEnum && Enum.IsDefined(type, value);
    }
}