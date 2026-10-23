import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

param kind kindType

type kindType = 'app' | 'functionapp'

type skuType = {
  name: string
  tier: string
  size: string
  family: string
  capacity: int
}

var sku skuType = kind == 'app' ? {
  name: 'F1'
  tier: 'Free'
  size: 'F1'
  family: 'F'
  capacity: 0
} : kind == 'functionapp' ? {
  name: 'Y1'
  tier: 'Dynamic'
  size: 'Y1'
  family: 'Y'
  capacity: 0
} : {
  name: ''
  tier: ''
  size: ''
  family: ''
  capacity: 0
}

resource asp 'Microsoft.Web/serverfarms@2025-03-01' = {
  name: 'asp-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  kind: kind
  sku: sku
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
    asyncScalingEnabled: false
  }
}

output aspId string = asp.id
output aspName string = asp.name
