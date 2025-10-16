import {
  UseQueryResult,
  useQuery,
} from '@tanstack/react-query'
import { Setting } from '../models/Setting'

const queryKeys = {
  all: () => ['Settings'] as const,
  getSettings: () =>
    [[...queryKeys.all(), 'getSettings']] as const,
}

const API_PATH = process.env.REACT_APP_API_PATH
const API_VERSION = process.env.REACT_APP_API_VERSION

export interface SettingsApi {
  getSettings: () => UseQueryResult<Setting>
}

const fetchSettingsFn = async (): Promise<Setting> => {
  const response = await fetch(`${API_PATH}/${API_VERSION}/Settings`, {})
  if (response.ok) {
    const data = await response.json()
    return data as Setting
  }
  return {} as Setting
}
const useSettingsApi = (): SettingsApi => {
  const useGetSettings = () => {
    return useQuery<Setting>({
      queryKey: queryKeys.getSettings(),
      queryFn: () => fetchSettingsFn(),
      enabled: false,
    })
  }

  return {
    getSettings: useGetSettings,
  }
}

export default useSettingsApi
