import * as con from './modules/constants.bicep'

param env con.env
param system string = 'common'

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

