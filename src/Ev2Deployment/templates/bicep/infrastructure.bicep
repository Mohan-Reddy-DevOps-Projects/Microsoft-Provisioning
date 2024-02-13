param acrName string
param assistedIdAppObjectId string
param containerAppIdentityName string
param coreResourceGroupName string
param keyVaultName string
param location string = resourceGroup().location
param processingStorageSubscriptions array
param vnetName string

var contributorRoleDefName = 'b24988ac-6180-42a0-ab88-20f7382dd24c'
var keyVaultReaderRoleDefName = '21090545-7ca7-4776-b22c-e363652d74d2'
var keyVaultSecretsOfficerRoleDefName = 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
var keyVaultCertificatesUserRoleDefName = 'db79e9a7-68ee-4b58-9aeb-b90e7c24fcba'
var keyVaultCertificatesOfficerRoleDefName = 'a4417e6f-fecd-4de8-b567-7b0420556985'
var storageBlobDataContributorRoleDefName = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var storageTableDataContributorRoleDefName = '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'

resource containerAppIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: containerAppIdentityName
  location: location
}

resource acr 'Microsoft.ContainerRegistry/registries@2022-12-01' = {
  name: toLower(acrName)
  location: location
  sku: {
    name: 'Premium'
  }
  properties: {
    adminUserEnabled: true
    policies: {
      quarantinePolicy: {
        status: 'disabled'
      }
      trustPolicy: {
        type: 'Notary'
        status: 'disabled'
      }
      retentionPolicy: {
        days: 7
        status: 'enabled'
      }
    }
    dataEndpointEnabled: false
    publicNetworkAccess: 'Enabled'
    zoneRedundancy: 'Disabled'
  }
}

module acrRoleAssignments 'acrRoleAssignments.bicep' = {
  name: 'acrRoleAssignments'
  params: {
    acrName: acr.name
    principalId: containerAppIdentity.properties.principalId
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2023-04-01' existing = {
  name: vnetName
  scope: resourceGroup(coreResourceGroupName)
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  location: location
  name: keyVaultName
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
  }
}

module keyVaultReaderRoleModule 'keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultReaderRoleDeploy'
  params: {
    keyVaultName: keyVault.name
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: keyVaultReaderRoleDefName
  }
}

module keyVaultSecretsOfficerRoleModule 'keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultSecretsOfficerRoleDeploy'
  params: {
    keyVaultName: keyVault.name
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: keyVaultSecretsOfficerRoleDefName
  }
}

module keyVaultCertUserRoleModule 'keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultCertUserRoleDeploy'
  params: {
    keyVaultName: keyVault.name
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: keyVaultCertificatesUserRoleDefName
  }
}

// The assisted ID app needs Key Vault Certificate Officer to create certificates.
module keyVaultCertificatesOfficerRoleModule 'keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultCertificatesOfficerRoleDeploy'
  params: {
    keyVaultName: keyVault.name
    principalId: assistedIdAppObjectId
    roleDefinitionName: keyVaultCertificatesOfficerRoleDefName
  }
}

module processingStorageSubContributorRoleModule 'subscriptionRoleAssignment.bicep' = [for processingStorageSubscription in processingStorageSubscriptions: {
  name: 'processingStorageSubContributorRoleModuleDeploy_${processingStorageSubscription.stamp}'
  scope: subscription(processingStorageSubscription.id)
  params: {
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: contributorRoleDefName
    subscriptionId: processingStorageSubscription.id
  }
}]

module processingStorageSubBlobDataContributorRoleModule 'subscriptionRoleAssignment.bicep' = [for processingStorageSubscription in processingStorageSubscriptions: {
  name: 'processingStorageSubBlobDataContributorRoleModuleDeploy_${processingStorageSubscription.stamp}'
  scope: subscription(processingStorageSubscription.id)
  params: {
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: storageBlobDataContributorRoleDefName
    subscriptionId: processingStorageSubscription.id
  }
}]

module processingStorageSubTableDataContributorRoleModule 'subscriptionRoleAssignment.bicep' = [for processingStorageSubscription in processingStorageSubscriptions: {
  name: 'processingStorageSubTableDataContributorRoleModuleDeploy_${processingStorageSubscription.stamp}'
  scope: subscription(processingStorageSubscription.id)
  params: {
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionName: storageTableDataContributorRoleDefName
    subscriptionId: processingStorageSubscription.id
  }
}]

output containerAppIdentityClientId string = containerAppIdentity.properties.clientId
