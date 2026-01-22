package com.example.ena.api

data class ReturnRefundContextDto(
    val orderId: String,
    val paymentId: String,
    val lineItems: List<RefundLineItemContextDto>,
    val delivery: RefundValueDto?,
)

data class RefundLineItemContextDto(
    val id: String,
    val name: String,
    val quantity: Int,
    val price: RefundValueDto,
)

data class RejectCustomerReturnRequest(
    val rejection: ReturnRejectionDto,
)

data class ReturnRejectionDto(
    val code: String,
    val reason: String?,
)

data class PaymentRefundRequest(
    val payment: PaymentIdDto,
    val reason: String,
    val lineItems: List<RefundLineItemDto>?,
    val delivery: RefundValueDto?,
    val sellerComment: String?,
)

data class PaymentIdDto(
    val id: String,
)

data class RefundLineItemDto(
    val id: String,
    val type: String,
    val quantity: Int?,
    val value: RefundValueDto?,
)

data class RefundValueDto(
    val amount: String,
    val currency: String,
)
