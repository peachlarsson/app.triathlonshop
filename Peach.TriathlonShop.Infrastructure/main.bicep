import * as cmn from './modules/common.bicep'

param env cmn.env
param system string

module app './modules/app.bicep' = {
  params: {
    env: env
    system: system
    netFrameworkVersion: 'v10.0'
  }
}

module func './modules/func.bicep' = {
  params: {
    env: env
    system: system
  }
}
