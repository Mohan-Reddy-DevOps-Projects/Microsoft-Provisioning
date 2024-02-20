namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using Microsoft.Azure.ProjectBabylon.Metadata.Models;

public static class AccountModelExtension
{
    public static bool IsValidProvisioningState(this AccountServiceModel accountModel)
    {
        return accountModel.ProvisioningState == ProvisioningState.Creating ||
            accountModel.ProvisioningState == ProvisioningState.Moving ||
            accountModel.ProvisioningState == ProvisioningState.Failed ||
            accountModel.ProvisioningState == ProvisioningState.Succeeded;
    }

    public static bool IsValidSubscriptionState(this SubscriptionModel subscriptionModel)
    {
        return subscriptionModel.SubscriptionState == SubscriptionState.Registered;
    }

    public static bool IsFreeTier(this AccountServiceModel accountModel)
    {
        return string.Equals(accountModel.Sku?.Name, AccountSkuName.Free, StringComparison.OrdinalIgnoreCase);
    }
}
