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
  }
}

module sqlDatabase './modules/sqldatabase.bicep' = {
  params: {
    env: env
    system: system
  }
}

