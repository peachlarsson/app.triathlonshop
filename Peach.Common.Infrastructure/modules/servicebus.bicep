import * as cmn from './constants.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

resource sbns 'Microsoft.ServiceBus/namespaces@2026-07-01-preview' = {
  name: 'sbns-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
}
