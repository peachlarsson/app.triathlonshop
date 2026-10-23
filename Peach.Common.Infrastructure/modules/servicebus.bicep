import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

resource sbns 'Microsoft.ServiceBus/namespaces@2026-07-01-preview' = {
  name: 'sbns-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
}
