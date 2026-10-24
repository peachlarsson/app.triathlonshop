import * as con from '../modules/constants.bicep'
import * as appConfig from '../modules/appconfiguration.bicep'

param env string
param system string

resource existingAppConfig 'Microsoft.AppConfiguration/configurationStores@2025-06-01-preview' = {
  name: 'appcs-${system}-${env}-${con.companyInfo.name}'
}

module appConfigModule '../modules/appconfiguration.bicep' = {
  params: {
    env: env
    system: system
    appSettingsList: [
      {
        key: 'exampleKey'
        value: 'exampleValue'
        contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
      }
      {
        key: 'anotherExampleKey'
        value: 'anotherExampleValue'
      }
    ]
  }
}
