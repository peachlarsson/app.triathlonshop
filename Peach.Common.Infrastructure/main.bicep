import * as con from './modules/constants.bicep'

param env con.env
param system string = 'common'

module appInsights './modules/appinsights.bicep' = {
  params: {
    env: env
    system: system
  }
}

module sqlServer './modules/sqlserver.bicep' = {
  params: {
    env: env
    system: system
  }
}

module sqlElasticPool './modules/sqlelasticpool.bicep' = {
  params: {
    env: env
    system: system
    sqlServerName: sqlServer.outputs.sqlServerName
  }
}

module sqlDatabase './modules/sqldatabase.bicep' = {
  params: {
    env: env
    system: system
    sqlServerName: sqlServer.outputs.sqlServerName
    sqlElasticPoolId: sqlElasticPool.outputs.sqlElasticPoolId
  }
}

module appConfig './modules/appconfiguration.bicep' = {
  params: {
    env: env
    system: system
  }
}

module keyVault './modules/keyvault.bicep' = {
  params: {
    env: env
    system: system
  }
}

module serviceBus './modules/servicebus.bicep' = {
  params: {
    env: env
    system: system
  }
}

module signalR './modules/signalr.bicep' = {
  params: {
    env: env
    system: system
  }
}
