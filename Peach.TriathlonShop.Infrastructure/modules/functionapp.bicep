import * as cmn from '../../Peach.Common.Infrastructure/modules/constants.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

var appName = 'func-${system}-${env}-${cmn.companyInfo.name}'

param netFrameworkVersion cmn.netFrameworkVersionType = 'v10.0'

resource functionApp 'Microsoft.Web/sites@2025-03-01' = {
  name: appName
  location: cmn.location
  tags: tags
  kind:  'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
   properties: {
    serverFarmId: asp.outputs.aspId
    siteConfig: {
      ipSecurityRestrictionsDefaultAction: 'Allow'
      scmIpSecurityRestrictionsDefaultAction: 'Allow'
      scmIpSecurityRestrictionsUseMain: true
      netFrameworkVersion: netFrameworkVersion
      alwaysOn: false
      http20Enabled: false
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
      }
      appSettings:  [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: '@Microsoft.AppConfiguration(Endpoint=https://appcs-common-dev-peach.azconfig.io; Key=Peach:ApplicationInsightsConnectionString; Label=common)'
        }
      ]
    }
   }
}

// App service plan
module asp '../../Peach.Common.Infrastructure/modules/appplan.bicep' = {
  name: 'module-asp-app'
  params: {
    env: env
    system: system
    kind: 'functionapp'
  }
}

// Application Insights
module appi '../../Peach.Common.Infrastructure/modules/appinsights.bicep' = {
  name: 'module-appi'
  params: {
    env: env
    system: system
  }
}

// Storage account
module stg './storageaccount.bicep' = {
  name: 'module-st'
  params: {
    env: env
    system: system
  }
}

// App configuration
module appconfig '../../Peach.Common.Infrastructure/modules/appconfiguration.bicep' = {
  name: 'module-appconfig'
  params: {
    env: env
    system: system
    tags: tags
  }
}

// SignalR
module signalR '../../Peach.Common.Infrastructure/modules/signalr.bicep' = {
  name: 'module-signalr'
  params: {
    env: env
    system: system
  }
}

// SQL database
module sqldb '../../Peach.Common.Infrastructure/modules/sqldatabase.bicep' = {
  name: 'module-sqldb'
  params: {
    env: env
    system: system
    tags: tags
  }
}

// Service Bus
module sbns '../../Peach.Common.Infrastructure/modules/servicebus.bicep' = {
  name: 'module-sbns'
  params: {
    env: env
    system: system
    tags: tags
  }
}
