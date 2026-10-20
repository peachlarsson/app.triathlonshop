import * as cmn from './common.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

var appName = 'app-${system}-${env}-${cmn.companyInfo.name}'
var commonRgName = 'rg-common-${env}-${cmn.companyInfo.name}'

param netFrameworkVersion cmn.netFrameworkVersionType = 'v10.0'

resource webApp 'Microsoft.Web/sites@2025-03-01' = {
  name: appName
  location: resourceGroup().location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: asp.outputs.aspId
    httpsOnly: true
    outboundVnetRouting: null
    siteConfig: {
      webSocketsEnabled: false
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      alwaysOn: true
      netFrameworkVersion: netFrameworkVersion
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      ipSecurityRestrictionsDefaultAction: 'Allow'
      scmIpSecurityRestrictionsDefaultAction: 'Allow'
      scmIpSecurityRestrictionsUseMain: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appi.outputs.appInsightsConnectionString
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: cmn.environment.long.capitalizedFirstLetter[env]
        }
        {
          name: 'AzureAppConfigEndpoint'
          value: 'https://appcs-common-${env}-${cmn.companyInfo.name}.azconfig.io'
        }
      ]
    }
  }
}

// App service plan
module asp './asp.bicep' = {
  name: 'module-asp-app'
  scope: resourceGroup(commonRgName)
  params: {
    env: env
    system: 'common'
    kind: 'app'
  }
}

// Application Insights
module appi './appi.bicep' = {
  name: 'module-appi'
  scope: resourceGroup(commonRgName)
  params: {
    env: env
    system: 'common'
  }
}

// App configuration
module appcs './appcs.bicep' = {
  name: 'module-appcs'
  scope: resourceGroup(commonRgName)
  params: {
    env: env
    system: 'common'
  }
}
