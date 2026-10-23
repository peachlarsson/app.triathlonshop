import * as cmn from './constants.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

param sku object = {
  name: 'standard'
  family: 'A'
}

resource keyVault 'Microsoft.KeyVault/vaults@2026-05-15' = {
  name: 'kv-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
  properties: {
    sku: sku
    tenantId: subscription().tenantId
  }
}
