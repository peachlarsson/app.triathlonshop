import * as cmn from '../Peach.Common.Infrastructure/modules/constants.bicep'

param env cmn.env
param system string

module app './modules/webapp.bicep' = {
  params: {
    env: env
    system: system
    netFrameworkVersion: 'v10.0'
  }
}

module func './modules/functionapp.bicep' = {
  params: {
    env: env
    system: system
  }
}

module storageAccountFunctionApp './modules/storageaccount.bicep' = {
  params: {
    env: env
    system: system
  }
}
