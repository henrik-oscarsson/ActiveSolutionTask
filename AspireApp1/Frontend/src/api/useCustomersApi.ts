import {
    UseQueryResult,
    useQuery,
} from '@tanstack/react-query'
import { Customer } from '../models/Customer'

const queryKeys = {
    all: () => ['customers'] as const,
    getCustomers: () =>
        [[...queryKeys.all(), 'getCustomers']] as const,
}

const API_PATH = process.env.REACT_APP_API_PATH
const API_VERSION = process.env.REACT_APP_API_VERSION

export interface CustomersApi {
    getCustomers: () => UseQueryResult<Array<Customer>>
}

const fetchCustomersFn = async (): Promise<Array<Customer>> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Customers`, {})
    if (response.ok) {
        const data = await response.json()
        return data as Array<Customer>
    }
    return []
}

const useCustomersApi = (): CustomersApi => {
    const useGetCustomers = () => {
        return useQuery<Array<Customer>>({
            queryKey: queryKeys.getCustomers(),
            queryFn: () => fetchCustomersFn(),
            enabled: false,
        })
    }

    return {
        getCustomers : useGetCustomers,
    }
}

export default useCustomersApi
