import * as cmn from './constants.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

param sku object = {
  name: 'Free'
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2025-08-01-preview' = {
  name: 'appcs-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
  sku: sku
}

output appConfigEndpoint string = appConfig.properties.endpoint
