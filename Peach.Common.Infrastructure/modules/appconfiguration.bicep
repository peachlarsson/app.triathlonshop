import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

type contentType = null | 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'

type appSettingType = {
  key: string
  value: string
  contentType: contentType
}

param sku object = {
  name: 'Free'
}

param appSettingsList appSettingType[] = []

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2025-08-01-preview' = {
  name: 'appcs-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  sku: sku
  properties: {
  }
}

resource appSettings 'Microsoft.AppConfiguration/configurationStores/keyValues@2025-08-01-preview' = [for setting in appSettingsList: {
  parent: appConfig
  name: setting.key
  properties: {
    value: setting.value
    contentType: setting.?contentType
  }
}]

output appConfigEndpoint string = appConfig.properties.endpoint
