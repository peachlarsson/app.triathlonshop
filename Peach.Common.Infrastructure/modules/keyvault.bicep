import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

param sku object = {
  name: 'standard'
  family: 'A'
}

resource keyVault 'Microsoft.KeyVault/vaults@2026-05-15' = {
  name: 'kv-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  properties: {
    sku: sku
    tenantId: subscription().tenantId
  }
}
