import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

param sku object = {
  name: 'Free'
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2025-08-01-preview' = {
  name: 'appcs-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  sku: sku
}

output appConfigEndpoint string = appConfig.properties.endpoint
