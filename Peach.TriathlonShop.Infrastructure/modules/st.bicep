import * as cmn from './common.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}
param sku object = {
  name: 'Standard_LRS'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2026-06-01' = {
  name: 'st${system}${env}${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
  sku: sku
  kind: 'StorageV2'
}

